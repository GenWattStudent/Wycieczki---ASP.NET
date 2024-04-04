class Timeline {
  constructor(mealsModel) {
    this.mealsModel = mealsModel
    this.timelineItem = $('.checkpoint')
    this.timeLineProgress = $('#timeline-progress')
    this.currentTimelineProgress = $('#current-timeline-progress')
    this.currentTimelineProgressIndicator = $('#current-timeline-progress-indicator')
    this.currentMeal = null
    this.progress = 0
  }

  init(meals) {
    this.timelineItem.each((index, element) => {
      const itemTop = element.offsetTop + element.offsetHeight / 2
      const lineDotIndicator = document.createElement('div')

      lineDotIndicator.classList.add('line-dot', 'bg-primary')
      lineDotIndicator.style.top = `${itemTop}px`

      this.timeLineProgress.append(lineDotIndicator)
    })

    const { currentMeal, progress } = this.getCurrentMealAndProgress(this.mealsModel.$values)
    this.currentMeal = currentMeal
    this.progress = progress

    this.updateTimelineProgress()
  }

  getTimelineProgress(meals) {
    const biggestDate = new Date(
      Math.max.apply(
        null,
        meals.map((meal) => new Date(meal.end))
      )
    )
    const smallestDate = new Date(
      Math.min.apply(
        null,
        meals.map((meal) => new Date(meal.start))
      )
    )
    const currentDate = new Date()

    const secondsPassed = (currentDate - smallestDate) / 1000
    const totalSeconds = (biggestDate - smallestDate) / 1000

    return (secondsPassed / totalSeconds) * 100
  }

  getCurrentMealAndProgress(meals) {
    const currentDate = new Date()
    let currentMeal = null
    let progress = 0

    for (let i = 0; i < meals.length; i++) {
      const mealStart = new Date(meals[i].start)
      const mealEnd = new Date(meals[i].end)

      if (currentDate >= mealStart && currentDate <= mealEnd) {
        currentMeal = meals[i]
        const secondsPassed = (currentDate - mealStart) / 1000
        const totalSeconds = (mealEnd - mealStart) / 1000
        progress = (secondsPassed / totalSeconds) * 100
        break
      }
    }

    return { currentMeal, progress }
  }

  updateTimelineProgress() {
    if (!this.currentMeal) {
      return
    }

    const currentMealIndex = this.mealsModel.$values.findIndex((meal) => meal.id === this.currentMeal.id)
    for (let i = 0; i < currentMealIndex; i++) {
      this.progress += 100 / this.mealsModel.$values.length
    }
    this.currentTimelineProgress.css('height', `${this.progress}%`)
    this.currentTimelineProgressIndicator.css('top', `${this.progress}%`)
  }
}
